import React from 'react';
import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score } from '../../types/dotnet-types';

interface Props {
    scoresUrl: string;
}

interface State {
    user: User;
    board: UserView;
    scores: Score[];
}

const ViewBoardComponent: React.FC<Props> = ({ scoresUrl }) => {
    const { isLoading, isMounted, reloadAsync, data } = useLoading<State>(
        scoresUrl,
        true
    );
    const { board, user, scores } = { ...data };
    const { unit } = { ...board?.uom };
    const { userName } = { ...user };

    // TODO display loading
    return (
        <div>
            <h4>Scores</h4>
            <p>hello {userName}!</p>
            {isMounted && !isLoading && unit != null ? (
                <ScoreTable
                    reloadAsync={reloadAsync}
                    scores={scores ?? []}
                    unit={unit}
                />
            ) : null}
        </div>
    );
};

export default ViewBoardComponent;
