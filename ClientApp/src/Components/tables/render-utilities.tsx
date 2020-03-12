import React, { PropsWithChildren, ReactNode } from 'react';

import flow from 'lodash/fp/flow';
import uniq from 'lodash/fp/uniq';
import join from 'lodash/fp/join';
import { CellProps } from 'react-table';

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
