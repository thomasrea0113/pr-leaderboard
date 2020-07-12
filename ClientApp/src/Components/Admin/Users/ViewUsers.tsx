import React, { useEffect } from 'react';
import { useLoading } from '../../../hooks/useLoading';
import { User } from '../../../types/dotnet-types';
import { isUser } from '../../../types/guards/isUser';
import { isArrayOf } from '../../../types/guards/higherOrderGuards';
import { UsersTable } from '../../tables/UsersTable';

const guard = isArrayOf(isUser);

export const ViewUsersComponent: React.FC<{}> = () => {
    const { response, loadAsync } = useLoading<User[]>({
        guard,
    });

    const getUsersAsync = () => {
        loadAsync({ actionUrl: '/api/Users' });
    };

    useEffect(getUsersAsync, []);

    const data =
        response?.data != null && guard(response.data) ? response.data : [];

    return <UsersTable users={data} />;
};
