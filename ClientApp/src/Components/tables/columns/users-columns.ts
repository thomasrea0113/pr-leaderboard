import { Column } from 'react-table';
import { User } from '../../../types/dotnet-types';

export const UsersColumns: Column<User>[] = [
    {
        Header: 'Username',
        id: 'user',
        accessor: u => u.userName,
    },
    {
        Header: 'Email',
        id: 'email',
        accessor: u => u.email,
    },
    {
        Header: 'Is Active',
        id: 'isActive',
        accessor: u => u.isActive,
        Cell: ({ cell: { value } }) => value.toString(),
    },
];
