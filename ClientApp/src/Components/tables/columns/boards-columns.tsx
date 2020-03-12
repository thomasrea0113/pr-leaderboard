import { Column } from 'react-table';
import React from 'react';
import { UserView } from '../../../types/dotnet-types';
import { GenderIcon, RangeDisplay } from '../../StyleComponents';
import { withSubRowCount, joinVals } from '../render-utilities';

export const BoardColumns: Column<UserView>[] = [
    {
        Header: 'Division Name',
        id: 'division-name',
        accessor: ({ division: { name } }) => name,
        Cell: c => withSubRowCount(c),
        aggregate: lv => joinVals(lv),
    },
    {
        Header: 'Categories',
        id: 'division-categories',
        accessor: ({ division: { categories } }) =>
            categories?.map(c => c.name),
        Cell: ({ cell: { value } }) => value?.join(', ') ?? '(none)',
        aggregate: lv => lv[0],
    },
    {
        Header: 'Gender',
        id: 'division-gender',
        accessor: ({ division: { gender } }) => gender,
        Cell: c => {
            const {
                cell: { value },
            } = c;
            return withSubRowCount(
                c,
                <>
                    &nbsp;
                    <GenderIcon gender={value} />
                </>
            );
        },
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => joinVals(lv),
    },
    {
        Header: 'Age Range',
        id: 'division-age',
        accessor: ({ division }) => [
            division.ageLowerBound,
            division.ageUpperBound,
        ],
        Cell: c => {
            const {
                cell: { value },
            } = c;
            return withSubRowCount(
                c,
                <RangeDisplay lowerBound={value[0]} upperBound={value[1]} />
            );
        },
        // the age is the same across all instances of the division, so we can just return the first value
        aggregate: lv => lv[0],
    },
    {
        Header: 'Board Name',
        id: 'board-name',
        accessor: r => r.name,
        disableGroupBy: true,
    },
    {
        Header: 'Unit of measure',
        id: 'board-uom',
        accessor: ({ uom: { unit } }) => unit,
        disableGroupBy: true,
    },
    {
        Header: 'Weight Lower Bound',
        id: 'board-weight-lower',
        accessor: ({ weightClass }) => weightClass?.weightLowerBound,
        disableGroupBy: true,
    },
    {
        Header: 'Weight Upper Bound',
        id: 'board-weight-upper',
        accessor: ({ weightClass }) => weightClass?.weightUpperBound,
        disableGroupBy: true,
    },
];
