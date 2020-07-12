import {
    TableOptions,
    PluginHook,
    TableInstance,
    useTable,
    SortingRule,
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

export const useDefaultTable = <T extends {}>(
    options: TableOptions<T>,
    ...plugins: PluginHook<T>[]
): TableInstance<T> => {
    const params = new URLSearchParams(window.location.search);

    const orderBy = params.getAll('orderBy');
    const orderDependencies = [orderBy.join('.')];

    const sortBy: SortingRule<T>[] = useMemo(() => {
        return orderBy.map(v => {
            return parseSort(v);
        });
    }, orderDependencies);

    const defaultOptions: TableOptions<T> = {
        autoResetSortBy: true,
        columns: [],
        data: [],
        initialState: {
            sortBy,
        },
    };

    const { data } = options;

    // options have to memoized (i think...), but we need to update the data when the
    // loading completes. So we memoize based on the search string params, and then
    // we allways append the latest data
    const mergedOptions = useMemo(
        () => mergeObjects(defaultOptions, options),
        orderDependencies
    );

    return useTable({ ...mergedOptions, data }, ...plugins);
};
