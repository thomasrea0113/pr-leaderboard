import React, { useState, useEffect } from 'react';
import 'react-dom';

import PropTypes from 'prop-types';
import { useTable } from 'react-table';

interface RecommendationsState {
    user: {
        userName: string;
        email: string;
        interests: {
            id: string;
            name: string;
        }[];
    };
}

interface RecommendationsProps {
    initialUrl: string;
}

const initialState: RecommendationsState = {
    user: {
        userName: '',
        email: '',
        interests: [],
    },
};

const RecommendationsComponent = (props: RecommendationsProps) => {
    const [
        {
            user: { interests },
        },
        setState,
    ] = useState(initialState);

    // load initial data
    useEffect(() => {
        const { initialUrl } = props;
        fetch(initialUrl)
            .then(response => response.json())
            .then(json => setState(json));
    }, []);

    const columns = [
        {
            Header: 'Interests',
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

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({
        columns,
        data: interests,
    });

    return (
        <div>
            <div>We think you&apos;ll find this information useful!</div>
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
