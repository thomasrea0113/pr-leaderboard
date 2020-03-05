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
                <div className="row">
                    {isMember ? (
                        <a href="?" className="btn btn-outline-success col-sm">
                            <strong>
                                <i className="fas fa-chevron-right" />
                                &nbsp; View Board
                            </strong>
                        </a>
                    ) : (
                        <form
                            className="col-sm"
                            method="post"
                            action="?handler=join"
                        >
                            {/* {HiddenFor('name', board)}
                            {HiddenFor('name', board.division, 'divisionName')}
                            {HiddenFor('weightLowerBound', board.weightClass)}
                            {HiddenFor('weightUpperBound', board.weightClass)} */}
                            <button
                                key="join-button"
                                type="submit"
                                name="join"
                                className="btn btn-outline-warning row col-sm"
                            >
                                <strong>
                                    <i className="fas fa-dumbbell" />
                                    &nbsp; Join Board
                                </strong>
                            </button>
                        </form>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Board;
