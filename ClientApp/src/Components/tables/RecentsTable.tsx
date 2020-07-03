import React, { useMemo, Fragment } from 'react';
import {
    Column,
    useTable,
    useGlobalFilter,
    useGroupBy,
    useSortBy,
    useExpanded,
    TableState,
    Row,
} from 'react-table';
import omit from 'lodash/fp/omit';
import { Score } from '../../types/dotnet-types';
import { ScoreColumns } from './columns/score-columns';
import { renderHeader, renderCell } from './render-utilities';

export interface RecentsTableProps {
    data: Score[];
}

const columns: Column<Score>[] = [
    ...ScoreColumns,
    {
        Header: 'Board',
        id: 'board',
        accessor: s => `${s.board.division.name} / ${s.board.name}`,
    },
];

export const RecentsTable: React.FC<RecentsTableProps> = ({ data }) => {
    const initialState: Partial<TableState<Score>> = useMemo(
        () => ({
            sortBy: [{ id: 'score', desc: true }],
        }),
        []
    );

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable(
        {
            data,
            columns,
            initialState,
            expandSubRows: true,
            autoResetExpanded: false,
        },
        useGlobalFilter,
        useGroupBy,
        useSortBy,
        useExpanded
    );

    const renderRow = (r: Row<Score>): React.ReactFragment => {
        prepareRow(r);
        const props = r.getRowProps();
        const { key } = props;
        return (
            <Fragment key={key}>
                <tr {...omit(['key'], props)}>
                    {r.cells.map(cell => renderCell(cell))}
                </tr>
                {r.isExpanded ? r.subRows.map(sr => renderRow(sr)) : null}
            </Fragment>
        );
    };

    return (
        <table className="table" {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => renderHeader(headerGroup))}
            </thead>
            <tbody {...getTableBodyProps()}>
                {rows.map(r => renderRow(r))}
            </tbody>
        </table>
    );
};
