import React from 'react';
import { Column, useTable } from 'react-table';
import UserView from '../serverTypes/UserView';

const Columns: Column<UserView>[] = [
    {
        Header: 'Name',
        accessor: r => r.name,
    },
];

const BoardTableComponent: React.FunctionComponent<{
    renderDivision: boolean;
    boards: UserView[];
}> = props => {
    const { boards } = props;

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({ columns: Columns, data: boards });

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

export default BoardTableComponent;
