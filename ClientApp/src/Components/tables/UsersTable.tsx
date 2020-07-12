import React, { useMemo, Fragment } from 'react';
import {
    TableState,
    useTable,
    useGlobalFilter,
    useGroupBy,
    useSortBy,
    useExpanded,
    Row,
} from 'react-table';
import omit from 'lodash/fp/omit';
import { User } from '../../types/dotnet-types';
import { UsersColumns as columns } from './columns/users-columns';
import { renderCell, renderHeader } from './render-utilities';

export interface UsersTableProps {
    users: User[];
}

export const UsersTable: React.FC<UsersTableProps> = ({ users }) => {
    const initialState: Partial<TableState<User>> = useMemo(
        () => ({
            sortBy: [{ id: 'user', desc: true }],
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
            data: users,
            columns,
            initialState,
            expandSubRows: false,
            autoResetExpanded: false,
        },
        useGlobalFilter,
        useGroupBy,
        useSortBy,
        useExpanded
    );

    const renderRow = (r: Row<User>): React.ReactFragment => {
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
