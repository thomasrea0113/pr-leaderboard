import { Leaderboard } from '../dotnet-types';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const isBoard = (obj: any): obj is Leaderboard => {
    const board = obj as Leaderboard;
    if (
        board != null &&
        typeof board.id === 'string' &&
        typeof board.name === 'string' &&
        typeof board.slug === 'string' &&
        typeof board.joinUrl === 'string' &&
        typeof board.viewUrl === 'string' &&
        typeof board.uom?.unit === 'string' &&
        typeof board.division?.name === 'string'
    )
        return true;
    return false;
};
