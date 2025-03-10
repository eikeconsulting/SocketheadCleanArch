import { View, SafeAreaView, TouchableOpacity } from 'react-native';
import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { NavigationContainer } from '@react-navigation/native';
import { Feed, LittleSteps, Login, Menu, Signup, Task } from '@app/screens';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import colors from '@app/colors';
import { Image, Text } from '@app/components';
import { Assets } from '@app/assets';
import { AddReflection } from '@app/screens/reflection';

export type RootStackParamsList = {
    Signup: undefined,
    Login: undefined
};

const Stack = createNativeStackNavigator<RootStackParamsList>();
const Tab = createBottomTabNavigator();
const headerShownFalse = { headerShown: false, orientation: 'portrait' }

const Routes = () => {
    return (
        <NavigationContainer>
            <Stack.Navigator screenOptions={{ headerShown: false, }}>
                <Stack.Screen name='Login' component={Login} />
                <Stack.Screen name='Signup' component={TabBar} />
            </Stack.Navigator>
        </NavigationContainer>
    );
}

const TabBar = () => {

    return (
        <Tab.Navigator screenOptions={headerShownFalse} tabBar={props => <Tabs {...props} />}>
            <Tab.Screen name='LittleSteps' component={LittleSteps} />
            <Tab.Screen name='Task' component={Task} />
            <Tab.Screen name='AddReflection' component={AddReflection} />
            <Tab.Screen name='Feed' component={Feed} />
            <Tab.Screen name='Menu' component={Menu} />
        </Tab.Navigator>
    )
}

const Tabs = ({ state, navigation }: any) => {

    return (
        <>
            <View style={{ paddingTop: 0 }}>
                <View style={{ flexDirection: 'row', justifyContent: 'space-between', backgroundColor: colors.white }}>
                    {state.routes.map((route: any, index: number) => {
                        const isFocused = state.index === index;
                        const onPress = () => {
                            index === 0 ?
                                navigation.navigate('LittleSteps')
                                : index === 1 ? navigation.navigate('Task')
                                    : index === 2 ? navigation.navigate('AddReflection')
                                        : index === 3 ? navigation.navigate('Feed')
                                            : navigation.navigate('Menu')
                        };

                        return (
                            <TouchableOpacity onPress={onPress} style={{ padding: 15 }}>
                                {index === 0 ?
                                    <Image source={Assets.trails} style={{ width: 30, height: 30 }} tintColor={isFocused ? colors.primaryRed : colors.black} />
                                    : index == 1 ?
                                        <Image source={Assets.bullseye} style={{ width: 30, height: 30 }} tintColor={isFocused ? colors.primaryRed : colors.black} />
                                        : index == 2 ?
                                            <Image source={Assets.plus} style={{ width: 30, height: 30 }} tintColor={isFocused ? colors.primaryRed : colors.black} />
                                            : index == 3 ?
                                                <Image source={Assets.stone} style={{ width: 30, height: 30 }} tintColor={isFocused ? colors.primaryRed : colors.black} />
                                                : <Image source={Assets.more} style={{ width: 30, height: 30 }} tintColor={isFocused ? colors.primaryRed : colors.black} />

                                }
                            </TouchableOpacity>
                        );
                    })}
                </View>
            </View>
            <SafeAreaView style={{ backgroundColor: colors.white }} />
        </>
    );
};


export default Routes;
