/* eslint-disable no-nested-ternary */
import React, {
    useState,
    useEffect,
    useMemo,
    ReactNode,
    ReactFragment,
    Fragment,
    PropsWithChildren,
} from 'react';
import 'react-dom';

import flow from 'lodash/fp/flow';
import uniq from 'lodash/fp/uniq';
import join from 'lodash/fp/join';

import {
    useTable,
    useGroupBy,
    useExpanded,
    useGlobalFilter,
    TableState,
    Column,
    Cell,
    Row,
    CellProps,
} from 'react-table';
import uniqueId from 'lodash/fp/uniqueId';
import {
    Expander,
    Range,
    Grouper,
    GenderIcon,
    Checkbox,
} from '../Components/StyleComponents';
import { UserView, User } from '../types/dotnet-types';
import Board from '../Components/Board';

interface ReactProps {
    initialUrl: string;
    userName: string;
}

interface ReactState {
    user?: User;
    recommendations: UserView[];
    isLoading: boolean;
}

const InitialState: ReactState = {
    recommendations: [],
    isLoading: true,
};

const joinVals = <T extends {}>(vals: T[]): string =>
    flow(uniq, join(', '))(vals);

const withSubRowCount = <T extends {}>(
    {
        row: {
            subRows: { length },
            groupByID,
        },
        cell: {
            value,
            isGrouped,
            isPlaceholder,
            column: { id },
        },
    }: PropsWithChildren<CellProps<T>>,
    cellComponent?: ReactNode
) => {
    // TODO should no display repeated value when grouping multiple columns
    const cellVal = isPlaceholder ? undefined : cellComponent ?? value;
    return groupByID === id && isGrouped && length > 0 ? (
        <>
            {cellVal} ({length})
        </>
    ) : (
        cellVal
    );
};

const columns: Column<UserView>[] = [
    {
        Header: 'Division Name',
        id: 'division-name',
        accessor: ({ division: { name } }) => name,
        Cell: c => withSubRowCount(c),
        aggregate: lv => joinVals(lv),
    },
    {
        Header: 'Categories',
        id: 'division-categories',
        accessor: ({ division: { categories } }) =>
            categories?.map(c => c.name),
        Cell: ({ cell: { value } }) => value?.join(', ') ?? '(none)',
        aggregate: lv => lv[0],
    },
    {
        Header: 'Gender',
        id: 'division-gender',
        accessor: ({ division: { gender } }) => gender,
        Cell: c => {
            const {
                cell: { value },
            } = c;
            return withSubRowCount(
                c,
                <>
                    &nbsp;
                    <GenderIcon gender={value} />
                </>
            );
        },
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => joinVals(lv),
    },
    {
        Header: 'Age Range',
        id: 'division-age',
        accessor: ({ division }) => [
            division.ageLowerBound,
            division.ageUpperBound,
        ],
        Cell: c => {
            const {
                cell: { value },
            } = c;
            return withSubRowCount(
                c,
                <Range lowerBound={value[0]} upperBound={value[1]} />
            );
        },
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => lv[0],
    },
    {
        Header: 'Board Name',
        id: 'board-name',
        accessor: r => r.name,
        disableGroupBy: true,
    },
    {
        Header: 'Unit of measure',
        id: 'board-uom',
        accessor: ({ uom: { unit } }) => unit,
        disableGroupBy: true,
    },
    {
        Header: 'Weight Lower Bound',
        id: 'board-weight-lower',
        accessor: ({ weightClass }) => weightClass?.weightLowerBound,
        disableGroupBy: true,
    },
    {
        Header: 'Weight Upper Bound',
        id: 'board-weight-upper',
        accessor: ({ weightClass }) => weightClass?.weightUpperBound,
        disableGroupBy: true,
    },
];

const renderCell = (cell: Cell<UserView>): React.ReactNode | null => {
    const {
        isGrouped,
        isAggregated,
        isPlaceholder,
        getCellProps,
        render,
        row: { isExpanded, getToggleRowExpandedProps },
    } = cell;

    let innerCell: ReactNode;
    if (isAggregated) innerCell = render('Aggregated');
    else if (isGrouped)
        innerCell = (
            <>
                <Expander
                    props={getToggleRowExpandedProps()}
                    isExpanded={isExpanded}
                />
                {render('Cell')}
            </>
        );
    else if (isPlaceholder) innerCell = null;
    else innerCell = render('Cell');

    return <td {...getCellProps()}>{innerCell}</td>;
};

const RecommendationsComponent = (props: ReactProps) => {
    const [state, setState] = useState(InitialState);
    const { recommendations } = state;

    const { initialUrl } = props;

    const initialState = useMemo(() => {
        const tableState: Partial<TableState<UserView>> = {
            groupBy: ['division-name'],
            hiddenColumns: [
                'board-name',
                'board-uom',
                'board-weight-lower',
                'board-weight-upper',
            ],
        };
        return tableState;
    }, []);

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
        state: { groupBy },
        toggleGroupBy,
        getToggleAllRowsExpandedProps,
        toggleAllRowsExpanded,
        isAllRowsExpanded,
        setGlobalFilter,
        visibleColumns,
        visibleColumns: { length: visibleColumnCount },
    } = useTable(
        { data: recommendations, columns, initialState, expandSubRows: false },
        useGlobalFilter,
        useGroupBy,
        useExpanded
    );

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json => {
                setState({
                    ...state,
                    ...json,
                    isLoading: false,
                });
                toggleAllRowsExpanded(true);
            });
    }, []);

    const renderRow = (row: Row<UserView>): ReactFragment => {
        prepareRow(row);

        const {
            isGrouped,
            isExpanded,
            subRows,
            getRowProps,
            cells,
            original,
        } = row;

        // calls to getRowProps includes the key for the react mapped array. We want the fragment to have
        // the key, but all other props should be shared amongst both rows.
        const rowProps = getRowProps();
        const { key } = rowProps;
        const rowPropsNoKey = { ...rowProps, key: undefined };

        if (isGrouped)
            if (isExpanded)
                return (
                    <Fragment key={key}>
                        <tr {...rowPropsNoKey}>
                            {cells.map(cell => renderCell(cell))}
                        </tr>
                        {subRows.map(subRow => renderRow(subRow))}
                    </Fragment>
                );
            else
                return (
                    <tr {...rowProps}>{cells.map(cell => renderCell(cell))}</tr>
                );

        return (
            <tr {...rowProps}>
                <td colSpan={visibleColumnCount}>
                    <Board {...original} />
                </td>
            </tr>
        );
    };

    const setGroupBy = ({
        target: { value },
    }: React.ChangeEvent<HTMLSelectElement>) => {
        groupBy.forEach(v => toggleGroupBy(v, false));
        toggleGroupBy(value, true);
    };

    const groupById = uniqueId('select');
    const expandId = uniqueId('check');

    return (
        <div>
            <div className="form-row mb-2">
                <div className="form-group col-md-6 form-row">
                    <label
                        className="col-md-3 col-form-label"
                        htmlFor={groupById}
                    >
                        Group By
                    </label>
                    <select
                        id={groupById}
                        value={groupBy[0]}
                        className="form-control col-md-9"
                        onChange={setGroupBy}
                    >
                        {visibleColumns
                            .filter(c => !c.disableGroupBy)
                            .map(({ id, Header }) => (
                                <option key={id} value={id}>
                                    {Header}
                                </option>
                            ))}
                    </select>
                </div>
                <div
                    {...getToggleAllRowsExpandedProps()}
                    className="form-group col-md-3"
                >
                    <Checkbox
                        id={expandId}
                        label="Expand All"
                        checked={isAllRowsExpanded}
                    />
                </div>
                <div className="form-group col-md-3">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Search"
                        onChange={({ target: { value } }) =>
                            setGlobalFilter(value)
                        }
                    />
                </div>
            </div>
            <table className="table" {...getTableProps()}>
                <thead>
                    {headerGroups.map(headerGroup => (
                        <tr {...headerGroup.getHeaderGroupProps()}>
                            {headerGroup.headers.map(column => (
                                <th {...column.getHeaderProps()}>
                                    <Grouper
                                        hidden={!column.canGroupBy}
                                        isGrouped={column.isGrouped}
                                    />
                                    &nbsp;{column.render('Header')}
                                </th>
                            ))}
                        </tr>
                    ))}
                </thead>
                <tbody {...getTableBodyProps()}>
                    {rows.map(row => renderRow(row))}
                </tbody>
            </table>
        </div>
    );
};

export default RecommendationsComponent;
