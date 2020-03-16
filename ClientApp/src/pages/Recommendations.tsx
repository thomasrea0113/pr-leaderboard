/* eslint-disable no-nested-ternary */
import React, {
    useMemo,
    ReactNode,
    ReactFragment,
    Fragment,
    useEffect,
} from 'react';
import 'react-dom';

import {
    useTable,
    useGroupBy,
    useExpanded,
    useGlobalFilter,
    TableState,
    Cell,
    Row,
} from 'react-table';
import uniqueId from 'lodash/fp/uniqueId';
import { BoardColumns as columns } from '../Components/tables/columns/boards-columns';
import {
    Expander,
    Grouper,
    Checkbox,
    RefreshButton,
} from '../Components/StyleComponents';
import { UserView, User } from '../types/dotnet-types';
import Board from '../Components/Board';
import { useLoading } from '../hooks/useLoading';

interface ServerData {
    user?: User;
    recommendations: UserView[];
}

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

const RecommendationsComponent: React.FC<{
    initialUrl: string;
}> = ({ initialUrl }) => {
    const { isLoading, reloadAsync, data } = useLoading<ServerData>(initialUrl);

    const recommendations = useMemo(() => data?.recommendations ?? [], [data]);

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
        toggleAllRowsExpanded,
        getToggleAllRowsExpandedProps,
        isAllRowsExpanded,
        setGlobalFilter,
        visibleColumns,
        visibleColumns: { length: visibleColumnCount },
    } = useTable(
        {
            data: recommendations,
            columns,
            initialState,
            expandSubRows: false,
            autoResetExpanded: false,
        },
        useGlobalFilter,
        useGroupBy,
        useExpanded
    );

    useEffect(() => {
        reloadAsync().then(() => {
            if (!isAllRowsExpanded && !isLoading) toggleAllRowsExpanded(true);
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
        <>
            <div className="form-row">
                <div className="col-sm">
                    <div className="form-group">
                        <div className="form-row">
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
                    </div>
                </div>
                <div className="col-sm">
                    <div className="form-group">
                        <Checkbox
                            checkProps={{
                                id: expandId,
                                checked: isAllRowsExpanded,
                            }}
                            wrapperProps={getToggleAllRowsExpandedProps()}
                            label="Expand All"
                        />
                    </div>
                </div>
                <div className="col-sm">
                    <div className="form-group">
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
            </div>
            <div className="form-group">
                <div className="form-row">
                    <RefreshButton
                        isLoading={isLoading}
                        onClick={reloadAsync}
                        className="col-sm"
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
        </>
    );
};

export default RecommendationsComponent;
