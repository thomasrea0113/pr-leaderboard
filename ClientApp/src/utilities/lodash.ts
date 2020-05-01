/**
 * wrappers for commonly uses lodash flows
 */

import flow from 'lodash/fp/flow';
import toPairs from 'lodash/fp/toPairs';
import map from 'lodash/fp/map';
import fromPairs from 'lodash/fp/fromPairs';

export const mapPairs = <T, V>(
    obj: Partial<Record<keyof T, V>>,
    callback: (item: [keyof T, V]) => [keyof T, V]
): Partial<Record<keyof T, V>> => flow(toPairs, map(callback), fromPairs)(obj);
