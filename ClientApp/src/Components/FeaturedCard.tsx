import React from 'react';
import { Featured } from '../types/dotnet-types';
import {
    FontawesomeIconToIcon,
    bootstrapColorClassToString,
} from './StyleComponents';

export const FeaturedCard: React.FC<Featured> = ({
    title,
    description,
    links,
    image,
}) => {
    return (
        <div className="card" style={{ width: '25vw' }}>
            <img src={image} className="card-img-top" alt={title} />
            <div className="card-body">
                <h5 className="card-title">{title}</h5>
                <p className="card-text">{description}</p>
                {links.map(({ url, label, addon, className }) => (
                    <a
                        href={url}
                        className={
                            className != null
                                ? bootstrapColorClassToString(className)
                                : ''
                        }
                    >
                        {addon != null ? (
                            <>{FontawesomeIconToIcon(addon)} </>
                        ) : null}
                        {label}
                    </a>
                ))}
            </div>
        </div>
    );
};
