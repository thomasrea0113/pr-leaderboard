import React, { useEffect, useRef } from 'react';

// JQuery is expected to be included outside of the component, so we don't
// need to import it here to use it
// import $ from 'jquery';

import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score, UnitOfMeasure } from '../../types/dotnet-types';
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
    const { isLoading, isLoaded, loadAsync, response } = useLoading<State>();
    const formRef = useRef<HTMLFormElement>(null);

    if (response?.errorData !== undefined) {
        // TODO display errors
    }

    // load on mount
    useEffect(() => {
        loadAsync({ actionUrl: scoresUrl });
    }, []);

    const initial = {
        board: {
            slug: '',
            // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
            uom: {
                unit: 'Kilograms',
            } as UnitOfMeasure,
        },
        user: undefined,
        scores: undefined,
    };

    const {
        board: {
            slug,
            uom: { unit },
        },
        user,
        scores,
    } = response?.data != null ? response.data : initial;

    const {
        formDispatch,
        formProps,
        fieldAttributes: fieldProps,
    } = useFetchForm({
        fieldAttributes,
        formRef,
    });

    useEffect(() => {
        if (isLoaded && !isLoading) {
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
    }, [isLoaded, isLoading]);

    if (!isLoaded || isLoading) return <>loading...</>;

    if (user == null) throw new Error('user not provided');

    const reloadAsync = () => loadAsync({ actionUrl: scoresUrl });

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
                ref={formRef}
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
