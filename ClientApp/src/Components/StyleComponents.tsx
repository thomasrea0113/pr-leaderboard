import React, {
    DetailedHTMLProps,
    ButtonHTMLAttributes,
    HTMLAttributes,
    InputHTMLAttributes,
    useRef,
    useState,
} from 'react';

import omit from 'lodash/fp/omit';
import merge from 'lodash/fp/merge';
import { TableSortByToggleProps, TableGroupByToggleProps } from 'react-table';
import {
    GenderValues,
    BootstrapColorClass,
    FontawesomeIcon,
    Unit,
} from '../types/dotnet-types';
import { NumberRange } from '../types/types';
import { FieldPropInfo } from './forms/Validation';

/**
 * the color to use when a give icon is active
 */
const activeClass = 'text-warning';
const inactiveClass = 'text-muted';

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
    props: DetailedHTMLProps<HTMLAttributes<HTMLSpanElement>, HTMLSpanElement>;
}> = ({ isExpanded, props }) => {
    const mergedProps = merge(props, {
        style: { minWidth: '1.5rem' },
        className: 'no-highlight',
    });
    return (
        <>
            {isExpanded ? (
                <i
                    {...merge(mergedProps, {
                        className: 'fas fa-chevron-down',
                    })}
                />
            ) : (
                <i
                    {...merge(mergedProps, {
                        className: 'fas fa-chevron-right',
                    })}
                />
            )}
            &nbsp;
        </>
    );
};

export const GenderIcon: React.FC<{
    gender?: GenderValues;
}> = ({ gender }) => {
    const male = <i className="fas fa-male text-blue" />;
    const female = <i className="fas fa-female text-pink" />;
    const both = (
        <>
            {male}
            {female}
        </>
    );

    if (gender == null) return both;

    if (gender === GenderValues.Male) return male;
    if (gender === GenderValues.Female) return female;

    return both;
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
                    <i
                        className={`fas fa-check fa-stack-1x fa-inverse ${activeClass}`}
                    />
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
    groupByProps?: TableGroupByToggleProps;
}> = ({ hidden, isGrouped, groupByProps }) => (
    <i
        {...groupByProps}
        hidden={hidden}
        className={`fas fa-layer-group ${
            isGrouped ? activeClass : inactiveClass
        }`}
    />
);

export interface TableSortProps {
    toggleSortProps: TableSortByToggleProps;
    sort: boolean | undefined;
}

export const Sorter: React.FC<TableSortProps> = ({ toggleSortProps, sort }) => (
    <span {...toggleSortProps}>
        <i
            className={`fa fa-caret-up fa-lg${
                sort === true ? ` ${activeClass}` : ` ${inactiveClass}`
            }`}
        />
        <i
            className={`fa fa-caret-down fa-lg${
                sort === false ? ` ${activeClass}` : ` ${inactiveClass}`
            }`}
        />
    </span>
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

export interface ValidatorProps {
    forProp: FieldPropInfo;
}

export const ValidatorFor: React.FC<ValidatorProps> = ({ forProp }) => {
    if (forProp.attributes.id == null)
        throw new Error('field must have an id to bind a validator');
    return (
        <span
            className="text-danger field-validation-error"
            data-valmsg-for={forProp.attributes.id}
            data-valmsg-replace="true"
        />
    );
};

export const LoadingIcon: React.FC<{
    isLoading: boolean;
    iconClass?: string;
}> = ({ isLoading, iconClass }) =>
    isLoading ? (
        <>
            <i
                className={`fas fa-sync-alt fa-spin ${iconClass ??
                    activeClass}`}
            />
            &nbsp;&nbsp;
        </>
    ) : null;

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
                } ${activeClass}`}
            />
            &nbsp;&nbsp;refresh
        </button>
    );
};

export interface UnitIconProps {
    forUnit: Unit;
    additionalProps?: React.DetailedHTMLProps<
        React.HTMLAttributes<HTMLElement>,
        HTMLElement
    >;
}
export const UnitIcon: React.FC<UnitIconProps> = ({
    forUnit,
    additionalProps,
}) => {
    const { className } = { ...additionalProps };
    const mergeProps = (cls: string) => {
        if (className != null)
            return {
                ...additionalProps,
                className: `${className} ${cls}`,
            };
        return {
            ...additionalProps,
            className: cls,
        };
    };

    switch (forUnit) {
        case Unit.Kilograms:
            return <i {...mergeProps('fas fa-weight-hanging')} />;
        case Unit.Meters:
            return <i {...mergeProps('fas fa-ruler')} />;
        case Unit.Seconds:
            return <i {...mergeProps('fas fa-stopwatch')} />;
        default:
            break;
    }
    return null;
};

export const bootstrapColorClassToString = (
    color: keyof typeof BootstrapColorClass,
    outline: boolean = false
) => `btn btn${outline ? '-outline-' : '-'}${color.toLowerCase()}`;

export const FontawesomeIconToIcon = (icon: keyof typeof FontawesomeIcon) => (
    <i className={FontawesomeIcon[icon].toLowerCase()} />
);

export const FileUploader: React.FC<React.DetailedHTMLProps<
    React.InputHTMLAttributes<HTMLInputElement>,
    HTMLInputElement
>> = props => {
    const { id } = props;
    const [, setState] = useState(1);

    const fileRef = useRef<HTMLInputElement>(null);

    // const setFileName = (files: FileList | null) => {
    //     if (files != null && files.length > 0) setState(files[0].name);
    // };

    const rerender = () => setState(os => os + 1);

    const fileName = (function getFileName() {
        // eslint-disable-next-line prefer-destructuring
        const files = fileRef.current?.files;
        if (files != null && files.length > 0) {
            return files[0].name;
        }
        return '';
    })();

    return (
        <>
            <table className="file-upload">
                <tbody>
                    <tr>
                        <td style={{ width: '1%' }}>
                            <label
                                htmlFor={id}
                                className="btn btn-outline-primary circle"
                            >
                                <div className="content">
                                    <div>
                                        <i className="fas fa-cloud-upload-alt fa-2x" />
                                    </div>
                                    <div>Click to Upload</div>
                                </div>
                            </label>
                        </td>
                        <td style={{ position: 'relative' }}>
                            <div>
                                <strong>Filename: </strong>
                            </div>
                            {fileName !== '' ? (
                                <>
                                    <i className="fas fa-check fa-lg text-success" />
                                    &nbsp;&nbsp;{fileName}
                                </>
                            ) : null}
                            <input
                                ref={fileRef}
                                id={id}
                                type="file"
                                hidden
                                onChange={rerender}
                            />
                            <button
                                hidden={fileName === ''}
                                type="button"
                                onClick={() => {
                                    if (fileRef.current != null) {
                                        fileRef.current.value = '';
                                        rerender();
                                    }
                                }}
                                className="btn btn-outline-danger btn-icon"
                                style={{
                                    top: '0.5rem',
                                    right: '0.5rem',
                                    position: 'absolute',
                                }}
                            >
                                <i className="fas fa-times fa-lg" />
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </>
    );
};
