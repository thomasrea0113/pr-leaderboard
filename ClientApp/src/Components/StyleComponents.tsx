import React from 'react';
import { Row } from 'react-table';

export const NoBoundIcon: React.FunctionComponent = () => (
    <i className="fas fa-infinity" />
);

export interface ExpanderProps<P extends {}> {
    row: Row<P>;
}

export type Expander = <T extends {}>() => React.FC<ExpanderProps<T>>;

export const ExpanderFactory: Expander = <P extends {}>() => {
    const expander: React.FC<ExpanderProps<P>> = ({ row }: { row: Row<P> }) => (
        <span key={row.id} {...row.getToggleRowExpandedProps()}>
            {row.isExpanded ? '👇' : '👉'}
        </span>
    );
    return expander;
};
