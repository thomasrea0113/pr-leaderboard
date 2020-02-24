import React, { useState, useEffect } from 'react';
import 'react-dom';

import PropTypes from 'prop-types';
import { useTable } from 'react-table';

interface Board {
    id: string;
    name: string;
}

interface RecommendationsState {
    isLoading: boolean;
    user: {
        userName: string;
        email: string;
        recommendations: {
            id: string;
            name: string;
        }[];
    };
    userBoards: Board[];
    recommendations: Board[];
}

interface RecommendationsProps {
    initialUrl: string;
    loadingSelector: string;
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

    const columns = [
        {
            Header: 'Recommendations',
            columns: [
                {
                    Header: 'Name',
                    accessor: 'name',
                },
                {
                    Header: 'Id',
                    accessor: 'id',
                },
            ],
        },
    ];

    // TODO add boolean indication of whether or no the user is currently a member
    // of that board
    const data = [...recommendations, ...userBoards];

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({
        columns,
        data,
    });

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
