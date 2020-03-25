import React, { useEffect } from 'react';
import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score } from '../../types/dotnet-types';
import { SubmitScoreForm } from '../../Components/SubmitScoreForm';
import { attachHashEvents } from '../../utilities/modal-hash';

interface Props {
    scoresUrl: string;
    submitScoreUrl: string;
}

interface State {
    user: User;
    board: UserView;
    scores: Score[];
}

const ViewBoardComponent: React.FC<Props> = ({ scoresUrl, submitScoreUrl }) => {
    const { isLoading, isMounted, reloadAsync, data } = useLoading<State>(
        scoresUrl,
        true
    );
    const { board, user, scores } = { ...data };
    const { unit } = { ...board?.uom };

    useEffect(() => {
        if (isMounted && !isLoading) attachHashEvents();
    }, [isMounted, isLoading]);

    // TODO display loading
    return (
        <>
            <h4>Scores</h4>
            <p>hello {user?.userName}!</p>
            {/* <button
                id="submit-score"
                type="button"
                data-toggle="modal"
                data-target="#score-modal"
                className="btn btn-primary"
            >
                <i className="far fa-clipboard" />
                &nbsp;&nbsp;
                <label htmlFor="submit-score">Submit a Score</label>
            </button> */}
            {isMounted && !isLoading && board != null && unit != null ? (
                <>
                    <div className="mb-1">
                        <SubmitScoreForm unit={unit} />
                    </div>
                    <ScoreTable
                        reloadAsync={reloadAsync}
                        scores={scores ?? []}
                        unit={board.uom.unit}
                    />
                    {/* <form
                        method="post"
                        action={submitScoreUrl}
                        id="score-modal"
                        tabIndex={-1}
                        role="dialog"
                        className="modal fade"
                    >
                        <div
                            className="modal-dialog modal-dialog-centered"
                            role="document"
                        >
                            <div className="modal-content">
                                <div className="modal-header">
                                    <h5
                                        className="modal-title"
                                        id="exampleModalLabel"
                                    >
                                        Submit a Score
                                    </h5>
                                    <button
                                        type="button"
                                        className="close"
                                        data-dismiss="modal"
                                        aria-label="Close"
                                    >
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                                <div className="modal-body">
                                    <SubmitScoreForm unit={unit} />
                                </div>
                                <div className="modal-footer">
                                    <button
                                        type="button"
                                        className="btn btn-secondary"
                                        data-dismiss="modal"
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="submit"
                                        className="btn btn-primary"
                                    >
                                        Submit
                                    </button>
                                </div>
                            </div>
                        </div>
                    </form> */}
                </>
            ) : null}
        </>
    );
};

export default ViewBoardComponent;
