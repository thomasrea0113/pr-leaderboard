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
import { useFetchForm } from '../../hooks/useFetchForm';
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

    const {
        board: {
            slug,
            uom: { unit },
        },
        user,
        scores,
    } = data ?? { board: { uom: { unit: 'Kilograms' } } };

    const {
        formDispatch,
        formProps,
        fieldAttributes: fieldProps,
    } = useFetchForm({
        fieldAttributes,
        initialValues: {
            boardSlug: 'SHOULD BE OVERWRITTEN',
            score: 'TEST SCORE',
        },
    });

    useEffect(() => {
        if (isMounted && !isLoading) {
            attachHashEvents();

            if (slug == null || user?.userName == null)
                throw new Error('Server returned invalid data');

            // after we're done loading, update the form fields
            formDispatch({
                type: 'UPDATE_FIELDS',
                fields: [
                    {
                        property: 'boardSlug',
                        value: slug,
                    },
                    {
                        property: 'userName',
                        value: user.userName,
                    },
                ],
            });
        }
    }, [isMounted, isLoading]);

    if (!isMounted || isLoading) return <>loading...</>;

    if (user == null) throw new Error('user not provided');

    // TODO display loading
    return (
        <>
            <h4>Scores</h4>
            <p>hello {user.userName}!</p>
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
