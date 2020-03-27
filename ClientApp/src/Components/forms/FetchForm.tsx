import React from 'react';
import merge from 'lodash/fp/merge';
import { parseCookie } from '../../Site';

export type ReactFormProps = React.DetailedHTMLProps<
    React.FormHTMLAttributes<HTMLFormElement>,
    HTMLFormElement
>;

export interface ClientValidationField {
    tagName: string;
    attributes: { [key: string]: string };
}

export type FieldProps<T> = {
    [key in keyof T]: ClientValidationField;
};

export const FetchForm: React.FC<ReactFormProps> = props => {
    const { children, action, method } = props;

    // const {} = useLoading(action, false, 'GET');

    const csrf = parseCookie().requestVerificationToken;

    if (csrf == null) throw new Error('csrf token not found');

    const ajaxProps: Partial<ReactFormProps> = {
        onSubmit: async e => {
            e.preventDefault();
            const target = e.target as HTMLFormElement;
            await fetch(action ?? '/', {
                method: method ?? 'get',
                headers: {
                    RequestVerificationToken: csrf,
                    'Content-Type': 'application/json',
                },
                body: new FormData(target),
            });
        },
    };

    const mergedProps = merge(props, ajaxProps);
    return <form {...mergedProps}>{children}</form>;
};

// export interface FetchFormProps<T> {
//     formProps: ReactFormProps;
//     fieldAttributes: FieldProps<T>;
// }

// /**
//  * a generic component that renders a give form
//  */
// export class FetchForm<T extends {}> extends React.Component<
//     FetchFormProps<T>
// > {
//     private inner: React.FC<FetchFormProps<T>>;

//     public constructor(props: FetchFormProps<T>) {
//         super(props);
//         this.inner = () => {
//             const {
//                 formProps: { children, action, method },
//             } = props;

//             // const {} = useLoading(action, false, 'GET');

//             const csrf = parseCookie().requestVerificationToken;

//             if (csrf == null) throw new Error('csrf token not found');

//             const ajaxProps: Partial<ReactFormProps> = {
//                 onSubmit: async e => {
//                     e.preventDefault();
//                     const target = e.target as HTMLFormElement;
//                     await fetch(action ?? '/', {
//                         method: method ?? 'get',
//                         headers: {
//                             RequestVerificationToken: csrf,
//                             'Content-Type': 'application/json',
//                         },
//                         body: new FormData(target),
//                     });
//                 },
//             };

//             const mergedProps = merge(props, ajaxProps);
//             return <form {...mergedProps}>{children}</form>;
//         };
//     }

//     public render() {
//         return this.inner(this.props);
//     }
// }
