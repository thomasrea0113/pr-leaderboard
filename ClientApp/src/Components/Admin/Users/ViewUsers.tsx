import React, { useEffect } from 'react';
import { useLoading } from '../../../hooks/useLoading';
import { User } from '../../../types/dotnet-types';
import { isUser } from '../../../types/guards/isUser';
import { isArrayOf } from '../../../types/guards/higherOrderGuards';
import { isValidationErrorResponseData } from '../../../types/ValidationErrorResponse';

export const ViewUsersComponent: React.FC<{}> = () => {
    const { response, loadAsync } = useLoading<User[]>({
        guard: isArrayOf(isUser),
    });

    const getUsersAsync = () => {
        loadAsync({ actionUrl: '/api/Users' });
    };

    useEffect(getUsersAsync, []);

    const data = isValidationErrorResponseData(response?.data)
        ? []
        : response?.data ?? [];

    return <>All Users</>;
};
