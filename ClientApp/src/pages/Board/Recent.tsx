import React, { useEffect, useState } from 'react';
import 'react-dom';

import { useLoading } from '../../hooks/useLoading';
import { Score } from '../../types/dotnet-types';
import { isScore } from '../../types/guards/isScore';
import { isArrayOf } from '../../types/guards/higherOrderGuards';

interface Props {
    initialUrl: string;
    top: number;
    refreshEvery: number;
}

type ServerData = Score[];

const guard = isArrayOf(isScore);

const RecentComponent: React.FC<Props> = ({
    initialUrl,
    top,
    refreshEvery,
}) => {
    const { loadAsync, isLoaded, isLoading, response } = useLoading({
        guard,
    });

    const [data, setData] = useState<Score[]>([]);

    let lastQuery = '';

    const loadScoresAsync = () =>
        loadAsync({
            actionUrl: `${initialUrl}&ApprovedSince=${lastQuery}`,
        });

    const reloadAsync = () => {
        const now = new Date().toJSON();
        const promise = loadScoresAsync().then(() => {
            lastQuery = now;
        });
        return promise;
    };

    useEffect(() => {
        loadScoresAsync();
        const interval = setInterval(() => reloadAsync(), refreshEvery);
        return () => clearInterval(interval);
    }, []);

    const newData =
        response?.data != null && guard(response.data) ? response.data : [];

    if (newData.length > 0)
        setData(oldData => [...newData, ...oldData].slice(top));

    return (
        <div>
            Recent Updates
            {isLoaded && !isLoading ? <>Loaded</> : null}
        </div>
    );
};

export default RecentComponent;
