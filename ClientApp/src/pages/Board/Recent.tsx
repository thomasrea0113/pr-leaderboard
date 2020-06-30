import React, { useEffect } from 'react';
import { useLoading } from '../../hooks/useLoading';
import { Score } from '../../types/dotnet-types';
import { isArrayOf } from '../../types/guards/isArrayOf';
import { isScore } from '../../types/guards/isScore';
import { isFeatured } from '../../types/guards/isFeatured';

interface Props {
    initialUrl: string;
}

type ServerData = Score[];

const RecentComponent: React.FC<Props> = ({ initialUrl }) => {
    const { loadAsync, isLoaded, isLoading, response } = useLoading({
        guard: isArrayOf(isFeatured(isScore)),
    });

    const reloadAsync = () => loadAsync({ actionUrl: initialUrl });

    useEffect(() => {
        reloadAsync();
    }, []);

    return (
        <div>
            Recent Updates
            {isLoaded && !isLoading ? <>Loaded</> : null}
        </div>
    );
};

export default RecentComponent;
