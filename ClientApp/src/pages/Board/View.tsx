import React, { useEffect } from 'react';
import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score } from '../../types/dotnet-types';
import {
    SubmitScoreForm,
    SubmitScore,
} from '../../Components/forms/SubmitScoreForm';
import { attachHashEvents } from '../../utilities/modal-hash';
import { useFetchForm } from '../../Components/forms/FetchForm';
import { FieldProps } from '../../Components/forms/Validation';

interface Props {
    scoresUrl: string;
    submitScoreUrl: string;
    fieldAttributes: FieldProps<SubmitScore>;
}

interface State {
    user: User;
    board: UserView;
    scores: Score[];
}

const ViewBoardComponent: React.FC<Props> = ({
    scoresUrl,
    submitScoreUrl,
    fieldAttributes,
}) => {
    const { isLoading, isMounted, reloadAsync, data } = useLoading<State>(
        scoresUrl,
        true
    );

    useEffect(() => {
        if (isMounted && !isLoading) attachHashEvents();
    }, [isMounted, isLoading]);

    const {
        board: {
            uom: { unit },
        },
        user,
        scores,
    } = data ?? { board: { uom: { unit: 'Kilograms' } } };

    const { formProps, fieldAttributes: fieldProps } = useFetchForm<
        SubmitScore
    >({
        fieldAttributes,
    });

    if (!isMounted || isLoading) return <>loading...</>;

    // TODO display loading
    return (
        <>
            <h4>Scores</h4>
            <p>hello {user?.userName}!</p>
            <form
                {...formProps}
                className="mb-1"
                method="post"
                action={submitScoreUrl}
            >
                <SubmitScoreForm fieldAttributes={fieldProps} unit={unit} />
                <button type="submit">Submit</button>
            </form>
            <ScoreTable
                reloadAsync={reloadAsync}
                scores={scores ?? []}
                unit={unit}
            />
        </>
    );
};

export default ViewBoardComponent;
