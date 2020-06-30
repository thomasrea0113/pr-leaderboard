import React, { useMemo, useEffect, useState } from 'react';

import {
    TableState,
    useTable,
    useGlobalFilter,
    useGroupBy,
    useSortBy,
    useExpanded,
    Column,
} from 'react-table';
import { useLoading } from '../../../hooks/useLoading';
import { Score } from '../../../types/dotnet-types';
import { renderHeader, getRowRender } from '../../tables/render-utilities';
import { ScoreColumns } from '../../tables/columns/score-columns';
import { useFetchForm } from '../../../hooks/useFetchForm';
import { HttpMethodsEnum } from '../../../types/types';
import { isValidationErrorResponseData } from '../../../types/ValidationErrorResponse';
import { isArrayOf } from '../../../types/guards/higherOrderGuards';
import { isScore } from '../../../types/guards/isScore';

interface ApproveScore {
    ids: string[];
}

export const ApproveScoreComponent: React.FC<{}> = () => {
    const [refresh, setRefresh] = useState(false);
    const triggerRefresh = () => setRefresh(s => !s);

    const { response, loadAsync, isLoaded, isLoading } = useLoading<Score[]>();

    const reloadAsync = () =>
        loadAsync({ actionUrl: '/api/Scores?isApproved=false' });

    useEffect(() => {
        reloadAsync();
    }, [refresh]);

    // data must be memo-ized, or it will cause a render loop
    const data = useMemo(
        () =>
            isLoaded &&
            !isLoading &&
            !isValidationErrorResponseData(response?.data)
                ? response?.data ?? []
                : [],
        [isLoaded, isLoading]
    );

    const initialState: Partial<TableState<Score>> = useMemo(
        () => ({
            sortBy: [{ id: 'user', desc: true }],
        }),
        []
    );

    const { formProps, SubmitButton, formDispatch } = useFetchForm<
        ApproveScore,
        Score[]
    >({
        actionUrl: '/api/Scores/Approve',
        actionMethod: HttpMethodsEnum.PATCH,
        onValidSubmit: triggerRefresh,
        guard: isArrayOf(isScore),
    });

    const approveColumns: Column<Score>[] = [
        ...ScoreColumns,
        {
            Header: 'Leaderboard',
            id: 'leaderboard',
            accessor: s => s.boardId,
        },
        {
            Header: 'Approve',
            id: 'approve',
            disableFilters: true,
            disableGroupBy: true,
            disableSortBy: true,
            accessor: s => s.id,
            Cell: ({ cell: { value } }) => (
                <SubmitButton
                    className="btn btn-success"
                    value={value}
                    onClick={() => {
                        formDispatch({
                            type: 'UPDATE_FIELD',
                            field: {
                                property: 'ids',
                                value: [value],
                            },
                        });
                        formDispatch({ type: 'SUBMIT_FORM' });
                    }}
                >
                    Approve
                </SubmitButton>
            ),
            aggregate: scores => scores,
            Aggregated: ({ cell: { value } }) => {
                return (
                    <SubmitButton
                        className="btn btn-success"
                        name="Ids"
                        value={value}
                        onClick={() => {
                            formDispatch({
                                type: 'UPDATE_FIELD',
                                field: {
                                    property: 'ids',
                                    value,
                                },
                            });
                            formDispatch({ type: 'SUBMIT_FORM' });
                        }}
                    >
                        Approve All
                    </SubmitButton>
                );
            },
        },
    ];

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable(
        {
            data,
            columns: approveColumns,
            initialState,
            expandSubRows: false,
            autoResetExpanded: false,
        },
        useGlobalFilter,
        useGroupBy,
        useSortBy,
        useExpanded
    );

    const renderRow = getRowRender(prepareRow);

    return (
        <form {...formProps}>
            <table className="table" {...getTableProps()}>
                <thead>
                    {headerGroups.map(headerGroup => renderHeader(headerGroup))}
                </thead>
                <tbody {...getTableBodyProps()}>
                    {rows.map(r => renderRow(r))}
                </tbody>
            </table>
        </form>
    );
};
