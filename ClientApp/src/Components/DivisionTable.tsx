/* eslint-disable max-classes-per-file */
import React, { useMemo } from 'react';

import {
    useTable,
    Column,
    useExpanded,
    useGroupBy,
    TableState,
} from 'react-table';
import UserView from '../serverTypes/UserView';
import { NoBoundIcon } from './StyleComponents';
import DivisionComponent from './Division';

const columns: Column<UserView>[] = [
    {
        Header: 'Division Information',
        id: 'division-information',
        columns: [
            {
                Header: 'Name',
                id: 'division-name',
                accessor: r => r.division.name,
            },
            {
                Header: 'Gender',
                accessor: r => r.division.gender ?? '(any)',
            },
            {
                Header: 'Age Range',
                accessor: r => (
                    <span>
                        {r.division.ageLowerBound ?? <NoBoundIcon />} -{' '}
                        {r.division.ageUpperBound ?? <NoBoundIcon />}
                    </span>
                ),
            },
        ],
    },
    {
        Header: 'Board Information',
        id: 'board-information',
        columns: [
            {
                Header: 'Name',
                accessor: r => r.name,
            },
        ],
    },
];

const DivisionTable: React.FunctionComponent<{
    boards: UserView[];
}> = props => {
    const { boards } = props;

    // must useMemo on the table data for useExpanded to work correctly
    const data = useMemo(() => boards, [boards]);

    const initialState = useMemo(() => {
        const state: Partial<TableState<UserView>> &
            Partial<TableState<object>> = {
            groupBy: ['division-information'],
        };
        return state;
    }, []);

    const { getTableProps, rows, prepareRow } = useTable(
        {
            columns,
            data,
            expandSubRows: false,
            initialState,
        },
        useGroupBy,
        useExpanded
    );

    return (
        <div className="container" {...getTableProps()}>
            {rows.map(row => {
                prepareRow(row);
                return <DivisionComponent key={row.id} row={row} />;
            })}
        </div>
    );
};

export default DivisionTable;
