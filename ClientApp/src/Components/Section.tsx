import React, { DetailedHTMLProps, ButtonHTMLAttributes } from 'react';

import flow from 'lodash/fp/flow';
import split from 'lodash/fp/split';
import drop from 'lodash/fp/drop';
import join from 'lodash/fp/join';

import { BootstrapColorClass } from '../types/dotnet-types';

export interface SectionProps {
    name: string;
    color: BootstrapColorClass;
}

export const pathToSectionId = (path: string) =>
    flow(split('/'), drop(1), join('.'))(path);

export const scrollToSection = (path: string) =>
    document.getElementById(pathToSectionId(path))?.scrollIntoView({
        behavior: 'smooth',
    });

export interface SectionLinkProps
    extends DetailedHTMLProps<
        ButtonHTMLAttributes<HTMLButtonElement>,
        HTMLButtonElement
    > {
    to: string;
}

export const ScrollToSectionButton: React.FC<SectionLinkProps> = props => {
    const { children, to } = props;
    return (
        <button {...props} type="button" onClick={() => scrollToSection(to)}>
            {children}
        </button>
    );
};

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
