import React, { DetailedHTMLProps, ButtonHTMLAttributes } from 'react';
import { BootstrapColorClass } from '../types/dotnet-types';

export interface SectionProps {
    name: string;
    color: BootstrapColorClass;
}

export interface SectionLinkProps
    extends DetailedHTMLProps<
        ButtonHTMLAttributes<HTMLButtonElement>,
        HTMLButtonElement
    > {
    to: string;
}

export const Section: React.FC<SectionProps> = ({ name, color, children }) => {
    return (
        <>
            <div id={name} className="anchor" />
            <div className="section container">
                <div className="header-text" style={{ fontSize: '2.5rem' }}>
                    {name}
                </div>
                <hr
                    className={`text-${BootstrapColorClass[
                        color
                    ].toLowerCase()}`}
                    style={{ borderBottom: '3px' }}
                />
                {children}
            </div>
        </>
    );
};
