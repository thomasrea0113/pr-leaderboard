import { User } from '../dotnet-types';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const isUser = (obj: any): obj is User => {
    const user = obj as User;
    if (
        user != null &&
        typeof user.id === 'string' &&
        typeof user.userName === 'string'
    )
        return true;
    return false;
};
