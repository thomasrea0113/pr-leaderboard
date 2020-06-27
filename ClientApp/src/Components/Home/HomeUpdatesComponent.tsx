import React from 'react';
import { Section } from '../Section';
import { BootstrapColorClass } from '../../types/dotnet-types';

export const HomeUpdatesComponent: React.FC<{}> = () => {
    return <Section name="Recent PRs" color={BootstrapColorClass.Primary} />;
};
