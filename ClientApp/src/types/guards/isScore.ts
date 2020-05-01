import { Score } from '../dotnet-types';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const isScore = (obj: any): obj is Score => {
    const score = obj as Score;
    if (
        score != null &&
        typeof score.id === 'string' &&
        typeof score.boardId === 'string' &&
        typeof score.userId === 'string' &&
        typeof score.value === 'number' &&
        typeof score.isApproved === 'boolean'
    )
        return true;
    return false;
};
