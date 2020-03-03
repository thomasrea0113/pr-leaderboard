/* eslint-disable no-nested-ternary */
import React, {
    useState,
    useEffect,
    useMemo,
    ReactNode,
    ReactFragment,
    Fragment,
} from 'react';
import 'react-dom';

import {
    useTable,
    useGroupBy,
    useExpanded,
    TableState,
    Column,
    Cell,
    Row,
} from 'react-table';
import {
    Expander,
    Range,
    Grouper,
    GenderIcon,
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

const columns: Column<UserView>[] = [
    {
        Header: 'Division Name',
        id: 'division-name',
        accessor: ({ division: { name } }) => name,
        Cell: ({
            row: {
                subRows: { length },
            },
            cell: { value },
        }) =>
            length > 0 ? (
                <>
                    {value} ({length})
                </>
            ) : (
                value
            ),
    },
    {
        Header: 'Gender',
        id: 'division-gender',
        accessor: ({ division: { gender } }) => gender ?? '(All Genders)',
        Cell: ({ cell: { value } }) => (
            <>
                &nbsp;
                <GenderIcon gender={value} />
            </>
        ),
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => lv[0],
        disableGroupBy: true,
    },
    {
        Header: 'Age Range',
        id: 'division-age',
        accessor: ({ division }) => division,
        Cell: ({
            cell: {
                value: { ageLowerBound, ageUpperBound },
            },
        }) => <Range lowerBound={ageLowerBound} upperBound={ageUpperBound} />,
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => lv[0],
        disableGroupBy: true,
    },
    {
        Header: 'Board Name',
        id: 'board-name',
        accessor: r => r.name,
        disableGroupBy: true,
    },
];

const renderCell = (cell: Cell<UserView>): React.ReactNode | null => {
    const {
        isGrouped,
        isAggregated,
        isRepeatedValue,
        row: { isExpanded, getToggleRowExpandedProps },
    } = cell;

    let innerCell: ReactNode;

    if (isAggregated) innerCell = cell.render('Aggregated');
    else if (isGrouped)
        innerCell = (
            <>
                <Expander
                    props={getToggleRowExpandedProps()}
                    isExpanded={isExpanded}
                />
                {cell.render('Cell')}
            </>
        );
    else if (isRepeatedValue) innerCell = null;
    else innerCell = cell.render('Cell');

    return <td {...cell.getCellProps()}>{innerCell}</td>;
};

const RecommendationsComponent = (props: ReactProps) => {
    const [state, setState] = useState(InitialState);
    const { recommendations } = state;

    const { initialUrl } = props;

    const initialState = useMemo(() => {
        const tableState: Partial<TableState<UserView>> = {
            groupBy: ['division-name'],
            hiddenColumns: ['board-name'],
        };
        return tableState;
    }, []);

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
        visibleColumns: { length: visibleColumnCount },
    } = useTable(
        { data: recommendations, columns, initialState, expandSubRows: false },
        useGroupBy,
        useExpanded
    );

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json =>
                setState({
                    ...state,
                    ...json,
                    isLoading: false,
                })
            );
    }, []);

    const renderRow = (row: Row<UserView>): ReactFragment => {
        prepareRow(row);

        const {
            isGrouped,
            isExpanded,
            groupByID,
            subRows,
            getRowProps,
            cells,
            original,
        } = row;

        let renderedCells: ReactFragment | undefined;
        if (isExpanded) {
            // if we're grouping by a division field, we want the expanded rows to be a board
            if (groupByID.startsWith('division-'))
                renderedCells = subRows.map(r => {
                    prepareRow(r);
                    const {
                        original: subRowOriginal,
                        getRowProps: getSubRowProps,
                    } = r;
                    return (
                        <tr {...getSubRowProps()}>
                            <td colSpan={visibleColumnCount}>
                                <Board {...subRowOriginal} />
                            </td>
                        </tr>
                    );
                });
            // if we're not grouped by division, we want to simply render the sub rows
            else renderedCells = subRows.map(subRow => renderRow(subRow));
        }

        // calls to getRowProps includes the key for the react mapped array. We want the fragment to have
        // the key, but all other props should be shared amongst both rows.
        const rowProps = getRowProps();
        const { key } = rowProps;
        const rowPropsNoKey = { ...rowProps, key: undefined };

        return (
            <Fragment key={key}>
                <tr {...rowPropsNoKey}>
                    {cells.map(cell => renderCell(cell))}
                </tr>
                {renderedCells}

                {/* render the original UserView as a board */}
                {!isGrouped ? (
                    <tr {...rowPropsNoKey}>
                        <td colSpan={visibleColumnCount}>
                            <Board {...original} />
                        </td>
                    </tr>
                ) : null}
            </Fragment>
        );
    };

    return (
        <table className="table" {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map(column => (
                            <th {...column.getHeaderProps()}>
                                <Grouper
                                    hidden={!column.canGroupBy}
                                    props={column.getGroupByToggleProps()}
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
    );
};

export default RecommendationsComponent;
