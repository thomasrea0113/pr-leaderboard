/* eslint-disable no-nested-ternary */
import React, { useState, useEffect, useMemo } from 'react';
import 'react-dom';

import {
    useTable,
    useGroupBy,
    useExpanded,
    TableState,
    Column,
} from 'react-table';
import User from '../serverTypes/User';
import UserView from '../serverTypes/UserView';
import {
    Expander,
    Range,
    Grouper,
    GenderIcon,
} from '../Components/StyleComponents';

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

const message = (loading: boolean, userName?: string): string => {
    if (loading)
        return `Hang tight, ${userName}! We're gathering some information for you.`;
    return "All done! Here's what we've got for you";
};

const columns: Column<UserView>[] = [
    {
        Header: 'Division Name',
        id: 'division-name',
        accessor: r => r.division.name,
    },
    {
        Header: 'Gender',
        accessor: r => r.division.gender ?? '(All Genders)',
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
        accessor: r => (
            <Range
                lowerBound={r.division.ageLowerBound}
                upperBound={r.division.ageUpperBound}
            />
        ),
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => lv[0],
        disableGroupBy: true,
    },
    {
        Header: 'Board Name',
        accessor: r => r.name,
        disableGroupBy: true,
        show: t => !t.columns.find(c => c.id === 'division-name')?.isGrouped,
    },
];

const RecommendationsComponent = (props: ReactProps) => {
    const [state, setState] = useState(InitialState);
    const { recommendations, isLoading } = state;

    const { initialUrl, userName } = props;

    const initialState = useMemo(() => {
        const tableState: Partial<TableState<UserView>> = {
            groupBy: ['division-name'],
        };
        return tableState;
    }, []);

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable(
        { data: recommendations, columns, initialState },
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

    return (
        <>
            <div>{message(isLoading, userName)}</div>
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
                    {rows.map(row => {
                        prepareRow(row);
                        return (
                            <tr {...row.getRowProps()}>
                                {row.cells.map(cell => {
                                    return (
                                        <td {...cell.getCellProps()}>
                                            {cell.isGrouped ? (
                                                // If it's a grouped cell, add an expander and row count
                                                <>
                                                    <Expander
                                                        props={row.getToggleRowExpandedProps()}
                                                        isExpanded={
                                                            row.isExpanded
                                                        }
                                                    />
                                                    {cell.render('Cell')} (
                                                    {row.subRows.length})
                                                </>
                                            ) : cell.isAggregated ? (
                                                // If the cell is aggregated, use the Aggregated
                                                // renderer for cell
                                                cell.render('Aggregated')
                                            ) : cell.isRepeatedValue ? null : ( // For cells with repeated values, render null
                                                // Otherwise, just render the regular cell
                                                cell.render('Cell')
                                            )}
                                        </td>
                                    );
                                })}
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </>
    );
};

export default RecommendationsComponent;
