import { Column } from 'react-table';
import { Score } from '../../../types/dotnet-types';

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
];
