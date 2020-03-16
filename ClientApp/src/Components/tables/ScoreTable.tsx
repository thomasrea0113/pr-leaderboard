import React, { Fragment, useMemo } from 'react';
import {
    useGlobalFilter,
    useTable,
    useGroupBy,
    useExpanded,
    Row,
    useSortBy,
    TableState,
} from 'react-table';
import omit from 'lodash/fp/omit';
import { ScoreColumns } from './columns/score-columns';
import { Score } from '../../types/dotnet-types';
import { Grouper } from '../StyleComponents';
import { renderCell } from './render-utilities';

interface LocalProps {
    reloadAsync?: () => Promise<void>;
    scores: Score[];
    unit: string;
}

export const ScoreTable: React.FC<LocalProps> = ({ scores, unit }) => {
    const initialState: Partial<TableState<Score>> = useMemo(
        () => ({
            sortBy: [{ id: 'score', desc: true }],
        }),
        []
    );

    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable(
        {
            data: scores,
            columns: ScoreColumns,
            initialState,
            expandSubRows: false,
            autoResetExpanded: false,
        },
        useGlobalFilter,
        useGroupBy,
        useSortBy,
        useExpanded
    );

    const renderRow = (r: Row<Score>): React.ReactFragment => {
        prepareRow(r);
        const props = r.getRowProps();
        const { key } = props;
        return (
            <Fragment key={key}>
                <tr {...omit(['key'], props)}>
                    {r.cells.map(cell => renderCell(cell))}
                </tr>
                {r.isExpanded ? r.subRows.map(sr => renderRow(sr)) : null}
            </Fragment>
        );
    };

    // because we wanted to load on mount, be must also check that we are mounted
    return (
        <table className="table" {...getTableProps()}>
            <thead>
                {headerGroups.map(headerGroup => (
                    <tr {...headerGroup.getHeaderGroupProps()}>
                        {headerGroup.headers.map(
                            ({
                                canGroupBy,
                                getHeaderProps,
                                getGroupByToggleProps,
                                isGrouped,
                                render,
                            }) => (
                                <th
                                    {...getHeaderProps()}
                                    {...getGroupByToggleProps()}
                                >
                                    <Grouper
                                        hidden={!canGroupBy}
                                        isGrouped={isGrouped}
                                    />
                                    &nbsp;{render('Header')}
                                </th>
                            )
                        )}
                    </tr>
                ))}
            </thead>
            <tbody {...getTableBodyProps()}>
                {rows.map(r => renderRow(r))}
            </tbody>
        </table>
    );
};
