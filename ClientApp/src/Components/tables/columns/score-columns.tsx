import { Column } from 'react-table';
import { Score } from '../../../types/dotnet-types';
import { renderDate } from '../render-utilities';

export const ScoreColumns: Column<Score>[] = [
    {
        Header: 'User',
        id: 'user',
        accessor: s => s.user.userName,
    },
    {
        Header: 'Score',
        id: 'score',
        accessor: s => s.value,
        disableGroupBy: true,
    },
    {
        Header: 'Submitted On',
        id: 'createdDate',
        accessor: s => s.createdDate,
        Cell: ({ cell: { value } }) =>
            value != null ? renderDate(value) : null,
        disableGroupBy: true,
    },
];
