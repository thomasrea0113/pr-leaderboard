import React, { ReactNode } from 'react';
import { Leaderboard, UserView } from '../types/dotnet-types';
import { Range, ThumbnailImage, GenderIcon } from './StyleComponents';

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
    uom: { unit },
    iconUrl,
    weightClass,
    isMember,
    division: { gender, ageLowerBound, ageUpperBound, categories },
}) => {
    const { weightLowerBound, weightUpperBound } = weightClass ?? {};
    const colClass = 'col-12 col-sm-6';

    const buttons = [
        <button
            key={isMember ? 'view-button' : 'join-button'}
            type="submit"
            name={!isMember ? 'view' : 'join'}
            className={`btn ${isMember ? 'btn-success' : 'btn-warning'} col-sm`}
        >
            {!isMember ? (
                <i className="fas fa-dumbbell" />
            ) : (
                <i className="fas fa-chevron-right" />
            )}
            &nbsp; {isMember ? 'View Board' : 'Join Board'}
        </button>,
    ];

    return (
        <div className="row p-1 rounded grow">
            <div className="col-md-3 hide-sm">
                <ThumbnailImage src={iconUrl} />
            </div>
            <div className="col-md-9 container">
                <div className="row text-center p-2">
                    <div className={colClass}>
                        <Field label="Board Name">
                            <strong>{name}</strong>
                        </Field>
                    </div>
                    <div className={colClass}>
                        <Field label="Gender">
                            <GenderIcon gender={gender} />
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
                    <div className={colClass}>
                        <Field label="Age Range">
                            <Range
                                lowerBound={ageLowerBound}
                                upperBound={ageUpperBound}
                            />
                        </Field>
                    </div>
                    <div className={colClass}>
                        <Field label="Unit">{unit}</Field>
                    </div>
                    <div className={colClass}>
                        <Field
                            label="Categories"
                            // TODO move this to a single component so it can be reused
                            value={
                                categories?.map(c => c.name).join(', ') ??
                                '(none)'
                            }
                        />
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
