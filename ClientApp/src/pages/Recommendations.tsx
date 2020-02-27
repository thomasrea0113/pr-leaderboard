import React, { useState, useEffect } from 'react';
import 'react-dom';

import PropTypes from 'prop-types';
import { useTable, Column } from 'react-table';
import User from '../serverTypes/User';
import UserView from '../serverTypes/UserView';

export interface ReactProps {
    initialUrl: string;
    userName: string;
}

interface ReactState {
    user: User;
    recommendations: UserView[];

    // This is not loaded from the server. Only used in this component
    isLoading: boolean;
}

const initialState: ReactState = {
    isLoading: true,
    user: {
        userName: '',
        email: '',
        interests: [],
        leaderboards: [],
    },
    recommendations: [],
};

const infiniteIcon = <i className="fas fa-infinity" />;

const columns: Column<UserView>[] = [
    {
        Header: 'Division',
        accessor: r => r.division,
        columns: [
            {
                Header: 'Name',
                accessor: r => r.division.name,
            },
            {
                Header: 'Gender',
                accessor: r => r.division.gender ?? '(any)',
            },
            {
                Header: 'Age Range',
                accessor: r => (
                    <span>
                        {r.division.ageLowerBound ?? infiniteIcon} -{' '}
                        {r.division.ageUpperBound ?? infiniteIcon}
                    </span>
                ),
            },
        ],
    },
];

const RecommendationsComponent = (props: ReactProps) => {
    const [{ recommendations, isLoading }, setState] = useState(initialState);

    const { initialUrl, userName } = props;

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json => setState({ isLoading: false, ...json }));
    }, []);

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({ columns, data: recommendations });

    const message = (loading: boolean): string => {
        if (loading)
            return `Hang tight, ${userName}! We're gathering some information for you.`;
        return "All done! Here's what we've got for you";
    };

    return (
        <div>
            <div>{message(isLoading)}</div>
            <table {...getTableProps()}>
                <thead>
                    {headerGroups.map(headerGroup => (
                        <tr {...headerGroup.getHeaderGroupProps()}>
                            {headerGroup.headers.map(column => (
                                <th {...column.getHeaderProps()}>
                                    {column.render('Header')}
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
                                            {cell.render('Cell')}
                                        </td>
                                    );
                                })}
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
};

RecommendationsComponent.propTypes = {
    initialUrl: PropTypes.string.isRequired,
};

export default RecommendationsComponent;
