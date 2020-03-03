import React from 'react';
import { Leaderboard, UserView } from '../types/dotnet-types';
import { Range, ThumbnailImage } from './StyleComponents';

/**
 * a component that takes a leaderboard as it's props, an
 * optionally the aditional props defined on the user view
 */
const Board: React.FC<Partial<UserView> & Leaderboard> = ({
    name,
    isMember,
    iconUrl,
    weightClass,
}) => {
    const { weightLowerBound, weightUpperBound } = weightClass ?? {};
    return (
        <div className="row shadow p-1 bg-light rounded">
            <div className="col-md-4">
                <ThumbnailImage src={iconUrl} />
            </div>
            <div className="col-md-4">{name}</div>
            <div className="col-md-4">
                <Range
                    lowerBound={weightLowerBound}
                    upperBound={weightUpperBound}
                />
            </div>
        </div>
    );
};

export default Board;
