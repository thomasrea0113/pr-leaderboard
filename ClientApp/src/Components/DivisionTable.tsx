/* eslint-disable max-classes-per-file */
import React from 'react';
import 'react-dom';

import PropTypes from 'prop-types';
import { useTable, Column } from 'react-table';

import flow from 'lodash/fp/flow';
import groupBy from 'lodash/fp/groupBy';
import map from 'lodash/fp/map';
import UserView from '../serverTypes/UserView';
import Division from '../serverTypes/Division';
import NoBoundIcon from './NoBoundIcon';

class DivisionBoards {
    // eslint-disable-next-line no-useless-constructor
    public constructor(public division: Division, public boards: UserView[]) {}
}

const columns: Column<DivisionBoards>[] = [
    {
        Header: 'Division',
        columns: [
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
        ],
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
    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({ columns, data: groupByDivision(divisions) });

    return (
        <table {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map(column => (
                            <th {...column.getHeaderProps()}>
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
                        <tr {...row.getRowProps()}>
                            {row.cells.map(cell => {
                                return (
                                    <td {...cell.getCellProps()}>
                                        {cell.render('Cell')}
                                    </td>
                                );
                            })}
                        </tr>
                    );
                })}
            </tbody>
        </table>
    );
};

DivisionTable.propTypes = {
    boards: PropTypes.arrayOf(PropTypes.instanceOf(UserView).isRequired)
        .isRequired,
};

export default DivisionTable;
