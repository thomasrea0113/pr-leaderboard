import React, { DetailedHTMLProps, HTMLAttributes } from 'react';
import uniqueId from 'lodash/fp/uniqueId';
import merge from 'lodash/fp/merge';
import { Featured } from '../types/dotnet-types';
import {
    FontawesomeIconToIcon,
    bootstrapColorClassToString,
} from './StyleComponents';

type DivProps = DetailedHTMLProps<
    HTMLAttributes<HTMLDivElement>,
    HTMLDivElement
>;
export const FeaturedCard: React.FC<Featured & {
    divProps?: DivProps;
}> = ({ title, description, links, image, divProps }) => {
    const defaultClassName = 'card card-transparent';
    const defaultStyle: DivProps = { style: { width: '27.5vw' } };

    const props = merge(defaultStyle, divProps);
    const { className: cls } = props;
    props.className = [cls, defaultClassName].filter(Boolean).join(' ');

    return (
        <div {...props}>
            <img src={image} className="card-img-top" alt={title} />
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
            </div>

            {links.length > 0 ? (
                <div className="card-footer">
                    <div className="row">
                        {links.map(({ url, label, addon, className }, i) => (
                            <div
                                key={uniqueId('link')}
                                className={`col-mg-12 col-lg${
                                    i !== links.length - 1
                                        ? ' mb-1 mb-lg-0'
                                        : ''
                                }`}
                            >
                                <a
                                    href={url}
                                    className={
                                        className != null
                                            ? bootstrapColorClassToString(
                                                  className,
                                                  true
                                              )
                                            : ''
                                    }
                                    style={{ width: '100%', height: '100%' }}
                                >
                                    <span>
                                        {addon != null ? (
                                            <>{FontawesomeIconToIcon(addon)} </>
                                        ) : null}
                                        {label}
                                    </span>
                                </a>
                            </div>
                        ))}
                    </div>
                </div>
            ) : null}
        </div>
    );
};
