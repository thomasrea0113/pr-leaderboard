import React, {
    DetailedHTMLProps,
    ButtonHTMLAttributes,
    HTMLAttributes,
    InputHTMLAttributes,
} from 'react';

import omit from 'lodash/fp/omit';
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

type DivProps = DetailedHTMLProps<
    HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
>;

type InputProps = DetailedHTMLProps<
    InputHTMLAttributes<HTMLInputElement>,
    HTMLInputElement
>;

export interface CheckboxProps {
    wrapperProps: DivProps;
    checkProps: InputProps;
    label: string;
}

export const Checkbox: React.FC<CheckboxProps> = ({
    wrapperProps,
    checkProps: { checked, id, value },
    label,
}) => {
    const { className, style } = wrapperProps;
    const defaultClassName = 'form-check';
    return (
        <span
            {...wrapperProps}
            style={{ ...style, display: 'initial' }}
            className={
                className != null
                    ? `${className} ${defaultClassName}`
                    : defaultClassName
            }
        >
            <span id={id} className="form-check-input fa-stack">
                <i className="far fa-square fa-stack-2x text-muted" />
                {checked ? (
                    <i className="fas fa-check fa-stack-1x fa-inverse text-primary" />
                ) : null}
            </span>
            <label
                style={{ ...style, verticalAlign: 'sub' }}
                className="form-check-label"
                htmlFor={id}
            >
                {label}
            </label>
            <input
                hidden
                type="checkbox"
                value={value}
                defaultChecked={checked}
            />
        </span>
    );
};

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

export const RangeDisplay: React.FC<NumberRange & {
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
        <div className="h-100 embed-responsive embed-responsive-4by3">
            <img
                className="embed-responsive-item embed-responsive-cover rounded"
                alt="Board"
                src={src}
            />
        </div>
    );
};

export const RefreshButton: React.FC<DetailedHTMLProps<
    ButtonHTMLAttributes<HTMLButtonElement>,
    HTMLButtonElement
> & {
    isLoading: boolean;
}> = props => {
    const { isLoading, className } = props;
    const defaultClassName = 'btn btn-secondary';
    return (
        <button
            {...omit(['isLoading', 'className'], props)}
            type="button"
            className={
                className != null
                    ? `${defaultClassName} ${className}`
                    : defaultClassName
            }
            disabled={isLoading}
        >
            <i
                className={`fas fa-sync-alt${
                    isLoading ? ' fa-spin' : ''
                } text-primary`}
            />
            &nbsp;&nbsp;refresh
        </button>
    );
};
