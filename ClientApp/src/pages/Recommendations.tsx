import React, { useState, useEffect } from 'react';
import 'react-dom';

import PropTypes from 'prop-types';
import { useTable, TableOptions } from 'react-table';

import flow from 'lodash/fp/flow';
import groupBy from 'lodash/fp/groupBy';

import Leaderboard from '../serverTypes/Leaderboard';
import User from '../serverTypes/User';
import Division from '../serverTypes/Divisiond';

interface LeaderboardUserView extends Leaderboard {
    isMember: boolean;
}
interface DivisionUserView extends Division {
    boards: LeaderboardUserView[];
}

interface RecommendationsProps {
    initialUrl: string;
    loadingSelector: string;
}

interface RecommendationsState {
    isLoading: boolean;
    user: User;
    userBoards: Leaderboard[];
    recommendations: Leaderboard[];
}

const initialState: RecommendationsState = {
    isLoading: true,
    user: {
        userName: '',
        email: '',
        recommendations: [],
    },
    userBoards: [],
    recommendations: [],
};
const fn: (
    numbers: Leaderboard[],
    isMember: boolean
) => DivisionUserView[] = flow(groupBy((b: Leaderboard) => b.divisionId));

const RecommendationsTable: TableOptions<DivisionUserView> = {
    columns: [
        {
            Header: 'Division',
            columns: [
                {
                    Header: 'Name',
                    accessor: 'division.name',
                },
                {
                    Header: 'Is Member',
                    accessor: '.isMember',
                },
            ],
        },
    ],
    data: [],
};

const RecommendationsComponent = (props: RecommendationsProps) => {
    const [{ userBoards, recommendations, isLoading }, setState] = useState(
        initialState
    );

    const { initialUrl } = props;

    // load initial data
    useEffect(() => {
        fetch(initialUrl)
            .then(response => response.json())
            .then(json => setState({ isLoading: false, ...json }));
    }, []);

    // if (!isLoading)
    //     RecommendationsTable.data = [
    //         ...fn(recommendations, false),
    //         ...fn(userBoards, true),
    //     ];

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable(RecommendationsTable);

    const message = (loading: boolean): string => {
        if (loading)
            return "Hang tight! We're gathering some information for you.";
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
