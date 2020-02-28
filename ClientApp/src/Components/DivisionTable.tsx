/* eslint-disable max-classes-per-file */
import React, { useMemo } from 'react';

import { useTable, Column, useExpanded } from 'react-table';

import flow from 'lodash/fp/flow';
import groupBy from 'lodash/fp/groupBy';
import map from 'lodash/fp/map';
import UserView from '../serverTypes/UserView';
import Division from '../serverTypes/Division';
import NoBoundIcon from './NoBoundIcon';
import BoardTable from './BoardTable';

class DivisionBoards {
    // eslint-disable-next-line no-useless-constructor
    public constructor(public division: Division, public boards: UserView[]) {}
}

const columns: Column<DivisionBoards>[] = [
    {
        // Make an expander cell
        Header: () => null, // No header
        id: 'expander', // It needs an ID
        Cell: ({ row }) => (
            // Use Cell to render an expander for each row.
            // We can use the getToggleRowExpandedProps prop-getter
            // to build the expander.
            <span key={row.id} {...row.getToggleRowExpandedProps()}>
                {row.isExpanded ? '👇' : '👉'}
            </span>
        ),
    },
    {
        Header: 'Name',
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
];

/**
 * returns the boards grouped by division
 * @param boards data retrieved from the server
 */
const groupByDivision = (boards: UserView[]): DivisionBoards[] =>
    flow(
        groupBy((b: UserView) => b.division.name),
        map(
            b =>
                (({
                    division: b[0].division,
                    boards: b,
                } as unknown) as DivisionBoards)
        )
    )(boards);

const DivisionTable: React.FunctionComponent<{
    boards: UserView[];
}> = props => {
    const { boards: divisions } = props;

    // must useMemo on the table data for useExpanded to work correctly
    const data = useMemo(() => groupByDivision(divisions), [divisions]);

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
        visibleColumns,
    } = useTable(
        {
            columns,
            data,
        },
        useExpanded
    );

    return (
        <table className="table" {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map(column => (
                            <th scope="col" {...column.getHeaderProps()}>
                                {column.render('Header')}
                            </th>
                        ))}
                    </tr>
                ))}
            </thead>
            <tbody {...getTableBodyProps()}>
                {rows.map(row => {
                    prepareRow(row);
                    return (
                        <React.Fragment key={`${row.id}`}>
                            <tr {...row.getRowProps()}>
                                {row.cells.map(cell => {
                                    return (
                                        <td {...cell.getCellProps()}>
                                            {cell.render('Cell')}
                                        </td>
                                    );
                                })}
                            </tr>
                            {row.isExpanded ? (
                                <tr>
                                    <td colSpan={visibleColumns.length}>
                                        <BoardTable
                                            boards={row.original.boards}
                                            renderDivision={false}
                                        />
                                    </td>
                                </tr>
                            ) : null}
                        </React.Fragment>
                    );
                })}
            </tbody>
        </table>
    );
};

export default DivisionTable;
