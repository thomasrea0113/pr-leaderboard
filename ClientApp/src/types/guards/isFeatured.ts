import { Featured } from '../types';
import { nameof } from '../../utilities/reflection';

export const isFeatured = <T>(
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    guard: (obj: any) => obj is T
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
) => (obj: any): obj is Featured<T> =>
    guard(obj) && nameof<Featured<T>>('isFeatured') in obj;
