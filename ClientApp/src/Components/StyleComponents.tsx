import React from 'react';

import { GenderValues } from '../types/dotnet-types';
import { NumberRange } from '../types/types';

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
    gender?: GenderValues;
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

export const Checkbox: React.FC<{
    id: string;
    label: string;
    checked: boolean;
}> = ({ checked, id, label }) => (
    <div>
        <span id={id} className="fa-stack">
            <i className="far fa-square fa-stack-1x text-muted" />
            {checked ? (
                <i className="fas fa-times fa-stack-1x fa-inverse text-primary" />
            ) : null}
        </span>
        <label htmlFor={id}>{label}</label>
        <input hidden type="checkbox" />
    </div>
);

export const Grouper: React.FC<{
    hidden?: boolean;
    isGrouped: boolean;
    props?: unknown;
}> = ({ hidden, isGrouped, props }) => (
    <i
        {...(props !== undefined ? props : {})}
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

export const ThumbnailImage: React.FC<{
    src: string;
}> = ({ src }) => {
    return (
        <div className="h-100 embed-responsive embed-responsive-4x3">
            <img
                className="embed-responsive-item embed-responsive-cover rounded"
                alt="Board"
                src={src}
            />
        </div>
    );
};
