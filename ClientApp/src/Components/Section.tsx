import React from 'react';

export interface SectionProps {
    name: string;
}

export const Section: React.FC<SectionProps> = ({ name, children }) => {
    return (
        <div className="section container">
            <div className="header-text" style={{ fontSize: '2.5rem' }}>
                {name}
            </div>
            <hr className="text-primary" style={{ borderBottom: '3px' }} />
            {children}
        </div>
    );
};
