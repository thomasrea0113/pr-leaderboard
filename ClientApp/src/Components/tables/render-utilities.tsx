import React, { PropsWithChildren, ReactNode, Fragment } from 'react';

import flow from 'lodash/fp/flow';
import uniq from 'lodash/fp/uniq';
import join from 'lodash/fp/join';
import { CellProps, Cell, HeaderGroup, Row } from 'react-table';
import omit from 'lodash/fp/omit';
import { Expander, Grouper, Sorter } from '../StyleComponents';

export const joinVals = <T extends {}>(vals: T[]): string =>
    flow(uniq, join(', '))(vals);

export const withSubRowCount = <T extends {}>(
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

export const renderHeader = <T extends {}>(
    header: HeaderGroup<T>,
    disableGroupBy = false
) => {
    return (
        <tr {...header.getHeaderGroupProps()}>
            {header.headers.map(
                ({
                    canGroupBy,
                    canSort,
                    isSortedDesc,
                    getSortByToggleProps,
                    getHeaderProps,
                    getGroupByToggleProps,
                    isGrouped,
                    render,
                }) => (
                    <th {...getHeaderProps()}>
                        {canGroupBy ? (
                            <>
                                <Grouper
                                    isGrouped={isGrouped}
                                    groupByProps={
                                        !disableGroupBy
                                            ? getGroupByToggleProps()
                                            : undefined
                                    }
                                />
                                &nbsp;
                            </>
                        ) : null}
                        {canSort ? (
                            <>
                                <Sorter
                                    toggleSortProps={getSortByToggleProps()}
                                    sort={isSortedDesc}
                                />
                                &nbsp;
                            </>
                        ) : null}
                        {render('Header')}
                    </th>
                )
            )}
        </tr>
    );
};

export const renderCell = <T extends {}>(
    cell: Cell<T>
): React.ReactNode | null => {
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

// render row is dependent on the prepareRow function returned by useTable, so this
// method simply returns a render function using the passed in prepare fuction
/**
 * render row is dependent on the prepareRow function returned by useTable,
 * so this method simply returns a render function using the passed in prepare fuction
 * @param prepareRow method returned by the useTable hook
 */
export const getRowRender = <T extends {}>(prepareRow: (row: Row<T>) => void) =>
    function renderRow(r: Row<T>): React.ReactFragment {
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
