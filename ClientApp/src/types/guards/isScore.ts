import { Score } from '../dotnet-types';
import { isUser } from './isUser';
import { isBoard } from './isBoard';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const isScore = (obj: any): obj is Score => {
    const score = obj as Score;
    if (
        score != null &&
        typeof score.id === 'string' &&
        isBoard(score.board) &&
        isUser(score.user) &&
        typeof score.value === 'number'
    )
        return true;
    return false;
};
