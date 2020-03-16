import React from 'react';
import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { Leaderboard, Score } from '../../types/dotnet-types';
import { useLoading } from '../../hooks/useLoading';

interface Props {
    board: Leaderboard;
    scoresUrl: string;
}

interface State {
    isLoading: boolean;
}

const ViewBoardComponent: React.FC<Props> = ({
    scoresUrl,
    board: {
        uom: { unit },
    },
}) => {
    const { isLoading, isMounted, reloadAsync, data } = useLoading<Score[]>(
        scoresUrl,
        true
    );

    return (
        <div>
            <h4>Scores</h4>
            {isMounted && !isLoading ? (
                <ScoreTable
                    reloadAsync={reloadAsync}
                    scores={data ?? []}
                    unit={unit}
                />
            ) : null}
        </div>
    );
};

export default ViewBoardComponent;
