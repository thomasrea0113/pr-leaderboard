import {
    TableOptions,
    PluginHook,
    TableInstance,
    useTable,
    SortingRule,
    IdType,
    Filters,
} from 'react-table';
import { useMemo } from 'react';
import { mergeObjects } from '../../utilities/mergeObjects';

const parseSort = <T extends {}>(sortString: string): SortingRule<T> => {
    const firstChar = sortString[0];

    // if the first character is a minus, sort descending. otherwise, sort assending
    const desc = firstChar === '-';

    const id =
        firstChar === '-' || firstChar === '+'
            ? sortString.substr(1)
            : sortString;
    return {
        desc,
        id,
    };
};

const parseFilters = <T extends {}>(filters: string[]): Filters<T> =>
    filters.map(f => {
        const [id, value] = f.split('=');
        return { id, value };
    });

export const useDefaultTable = <T extends {}>(
    options: TableOptions<T>,
    ...plugins: PluginHook<T>[]
): TableInstance<T> => {
    const params = new URLSearchParams(window.location.search);

    const sortBy = params.getAll('orderBy').map(parseSort);
    const groupBy = params.getAll('groupBy');
    const filters = parseFilters(params.getAll('filterBy'));
    const globalFilter = params.getAll('globalFilter').join(',');

    const defaultOptions: TableOptions<T> = {
        autoResetSortBy: true,
        columns: [],
        data: [],
        initialState: {
            sortBy,
            groupBy,
            filters,
            globalFilter,
        },
    };

    const { data } = options;

    // options have to memoized (i think...), but we need to update the data when the
    // loading completes. So we memoize based on the search string params, and then
    // we allways append the latest data
    const mergedOptions = useMemo(() => mergeObjects(defaultOptions, options), [
        window.location.search,
    ]);

    return useTable({ ...mergedOptions, data }, ...plugins);
};
