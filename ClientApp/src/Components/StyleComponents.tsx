import React from 'react';
import { GenderValues } from '../types/server';

export const BoundIcon: React.FC<{
    bound?: number;
    iconClass?: string;
}> = ({ bound, iconClass }) => (
    <>
        {bound ?? (
            <i
                className={`fas fa-infinity text-muted ${iconClass ?? 'fa-sm'}`}
            />
        )}
    </>
);

export const Expander: React.FC<{
    isExpanded: boolean;
    props: unknown;
}> = ({ isExpanded, props }) => (
    <span className="no-highlight" {...props}>
        {isExpanded ? (
            <i className="fas fa-chevron-down" />
        ) : (
            <i className="fas fa-chevron-right" />
        )}
        &nbsp;
    </span>
);

export const GenderIcon: React.FC<{
    gender: GenderValues;
}> = ({ gender }) => {
    const male = <i className="fas fa-male text-blue" />;
    const female = <i className="fas fa-female text-pink" />;

    switch (gender) {
        case GenderValues.Male:
            return male;
        case GenderValues.Female:
            return female;
        default:
            return (
                <>
                    {male}
                    {female}
                </>
            );
    }
};

export const Grouper: React.FC<{
    hidden?: boolean;
    isGrouped: boolean;
    props: unknown;
}> = ({ hidden, isGrouped, props }) => (
    <i
        {...props}
        hidden={hidden}
        className={`fas fa-layer-group ${
            isGrouped ? 'text-primary' : 'text-muted'
        }`}
    />
);

export const Range: React.FC<NumberRange & {
    iconClass?: string;
}> = ({ lowerBound, upperBound, iconClass }) => (
    <>
        <BoundIcon bound={lowerBound} iconClass={iconClass} /> -{' '}
        <BoundIcon bound={upperBound} iconClass={iconClass} />
    </>
);
