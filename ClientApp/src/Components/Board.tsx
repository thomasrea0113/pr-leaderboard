import React, { ReactNode } from 'react';
import { Leaderboard, UserView } from '../types/dotnet-types';
import { Range, ThumbnailImage } from './StyleComponents';

const Field: React.FC<{
    label: string;
    value?: ReactNode;
}> = ({ label, value, children }) => (
    <>
        <strong>{label}:</strong>&nbsp;&nbsp;&nbsp;
        {value !== undefined ? value : children}
    </>
);

/**
 * a component that takes a leaderboard as it's props, an
 * optionally the aditional props defined on the user view
 */
const Board: React.FC<Partial<UserView> & Leaderboard> = ({
    name,
    iconUrl,
    weightClass,
    isMember,
}) => {
    const { weightLowerBound, weightUpperBound } = weightClass ?? {};
    const colClass = 'col-6';

    const buttons = [
        <button
            key={isMember ? 'join-button' : 'view-button'}
            type="submit"
            name={isMember ? 'join' : 'view'}
            className={`btn ${isMember ? 'btn-success' : 'btn-warning'} col-sm`}
        >
            {isMember ? (
                <i className="fas fa-dumbbell" />
            ) : (
                <i className="fas fa-chevron-right" />
            )}
            &nbsp; {isMember ? 'Join Board' : 'View Board'}
        </button>,
    ];

    return (
        <div className="row subrow shadow p-1 bg-light rounded">
            <div className="col-md-3 hide-sm">
                <ThumbnailImage src={iconUrl} />
            </div>
            <div className="col col-md-9 container">
                <div className="row text-center pl-4 pr-4 pt-2 pb-2">
                    <div className={colClass}>
                        <Field label="Board Name">
                            <strong className="text-primary">{name}</strong>
                        </Field>
                    </div>
                    <div className={colClass}>
                        <Field label="Weight Range">
                            <Range
                                lowerBound={weightLowerBound}
                                upperBound={weightUpperBound}
                            />
                        </Field>
                    </div>
                </div>
                <form className="row" method="post" action="">
                    {buttons.map(b => b)}
                </form>
            </div>
        </div>
    );
};

export default Board;
