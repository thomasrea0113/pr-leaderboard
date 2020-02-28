import Division from './Division';
import UnitOfMeasure from './UnitOfMeasure';
import WeightClass from './WeightClass';

export default class LeaderboardModel {
    // eslint-disable-next-line no-useless-constructor
    public constructor(
        public name: string,
        public uom: UnitOfMeasure,
        public division: Division,
        public weightClass: WeightClass
    ) {}
}
